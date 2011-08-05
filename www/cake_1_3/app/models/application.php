<?php

class Application extends AppModel {
	var $name = 'Application';
	
	var $actsAs = array('search');
	
	var $useTable = 'app';
	var $primaryKey = 'app_id';
	var $displayField = 'name';

	var $cdr_target;

	function expand(&$data) {
		$data['LaunchOptions'] = json_decode($data[$this->name]['launch_options'], true);
		$data['UserDefined'] = json_decode($data[$this->name]['user_defined'], true);
	
		if(isset($data['AppVersion'])) {
			foreach($data['AppVersion'] as $key => $version) {
				$data['AppVersion'][$key]['launch_option_ids'] = implode(', ', json_decode($version['launch_option_ids']));
			}
		}
	}
	
	function findCapture() {
			return $this->AppStateCapture->find('all', array('conditions' => array('app_id' => $this->app_id, 'cdr_id >' => $this->cdr_target), 'order' => 'cdr_id ASC'));
	}
	
	function getHistoryConditions() {
		if(!isset($this->cdr_target)) {
			$hist_condition = array('cdr_id_last' => null);
		} else {
			$hist_condition = array('cdr_id <=' => (int)$this->cdr_target, 'OR' => array('cdr_id_last >=' => (int)$this->cdr_target, 'cdr_id_last' => null));
		}
		
		return $hist_condition;
	}
	
	function bindImmediate($versions) {
		$hist_condition = $this->getHistoryConditions();
		
		$many =  array(
				'AppFilesystem' =>
					array (
						'className'			=> 'AppFilesystem',
						'foreignKey'			=> 'app_id',
						'conditions'			=> $hist_condition,
					),
				'AppVersion' =>
					array (
						'className'			=> 'AppVersion',
						'foreignKey'			=> 'app_id',
						'order'					=> 'version_id ASC',
						'conditions'			=> $hist_condition
					)
				);
				
		$models = array('AppFilesystem');
		
		if(!$versions) {
			unset($many['AppVersion']);
		} else {
			$models[] = 'AppVersion';
		}
		
		$this->linkModel($models);
		$this->bindModel(array('hasMany' => $many), false);
	}
	
	function bindCapture() {
		$this->linkModel(array('AppStateCapture'));
	}
	
	function bindMany() {
		$hist_condition = $this->getHistoryConditions();
		
		$this->linkModel(array('AppsSubs', 'Subscription'));
		
		$this->Subscription->virtualFields['cdr_id'] = 'AppsSubs.cdr_id';
		
		$this->Subscription->bindModel(array('hasOne'=>array('AppsSubs' => array('conditions' => $hist_condition) )), false);
	}
	
	function linkModel($model) {
		if(is_array($model)) {
			foreach($model as $m) $this->linkModel($m);
		} else {
			$this->{$model} = ClassRegistry::init(array('class' => $model, 'alias' => $model));
		}
	}
	
}

?>