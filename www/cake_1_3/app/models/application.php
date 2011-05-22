<?php

class Application extends AppModel {
	var $name = 'Application';
	
	var $actsAs = array('search');
	
	var $useTable = 'app';
	var $primaryKey = 'app_id';
	var $displayField = 'name';

	var $cdr_target;

	function findCapture() {
			return $this->AppStateCapture->find('all', array('conditions' => array('app_id' => $this->app_id, 'cdr_id >=' => $this->cdr_target), 'order' => 'cdr_id ASC'));
	}
	
	function bindImmediate($versions) {
		if(!isset($this->cdr_target)) {
			$hist_condition = array('cdr_id_last' => null);
		} else {
			$hist_condition = array('cdr_id <=' => (int)$this->cdr_target, 'OR' => array('cdr_id_last >=' => (int)$this->cdr_target, 'cdr_id_last' => null));
		}
		
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
		$this->linkModel(array('AppsSubs', 'Subscription'));
		
		$this->Subscription->bindModel(array('hasOne'=>array('AppsSubs')), false);
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