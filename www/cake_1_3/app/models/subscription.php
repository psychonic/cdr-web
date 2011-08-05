<?php

class Subscription extends AppModel {
	var $name = 'Subscription';
	
	var $actsAs = array('search');
	
	var $useTable = 'sub';
	var $primaryKey = 'sub_id';
	var $displayField = 'name';
	
	var $cdr_target;

	function expand(&$data) {
		$data['ExtendedInfo'] = json_decode($data[$this->name]['extended_info']);
	}
	
	function findCapture() {
			return $this->SubStateCapture->find('all', array('conditions' => array('sub_id' => $this->sub_id, 'cdr_id >' => $this->cdr_target), 'order' => 'cdr_id ASC'));
	}
	
	function bindCapture() {
		$this->linkModel(array('SubStateCapture'));
	}

	function getHistoryConditions() {
		if(!isset($this->cdr_target)) {
			$hist_condition = array('cdr_id_last' => null);
		} else {
			$hist_condition = array('cdr_id <=' => (int)$this->cdr_target, 'OR' => array('cdr_id_last >=' => (int)$this->cdr_target, 'cdr_id_last' => null));
		}
		
		return $hist_condition;
	}
	
	function bindMany() {
		$hist_condition = $this->getHistoryConditions();	
		
		$this->linkModel(array('AppsSubs', 'Application'));
		
		$this->Application->virtualFields['cdr_id'] = 'AppsSubs.cdr_id';
		
		$this->Application->bindModel(array('hasOne'=>array('AppsSubs' => array('conditions' => $hist_condition) )), false);
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