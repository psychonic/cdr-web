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
			return $this->SubStateCapture->find('all', array('conditions' => array('sub_id' => $this->sub_id, 'cdr_id >=' => $this->cdr_target), 'order' => 'cdr_id ASC'));
	}
	
	function bindCapture() {
		$this->linkModel(array('SubStateCapture'));
	}
	
	function bindMany() {
		$this->linkModel(array('AppsSubs', 'Application'));
		
		$this->Application->virtualFields['cdr_id'] = 'AppsSubs.cdr_id';
		
		$this->Application->bindModel(array('hasOne'=>array('AppsSubs')), false);
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