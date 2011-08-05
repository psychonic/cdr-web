<?php

class ContentRecord extends AppModel {
	var $name = 'ContentRecord';
	
	var $useTable = 'cdr';
	var $primaryKey = 'cdr_id';
	var $displayField = 'cdr_id';

	function bindCapture() {
		$this->linkModel(array('AppStateCapture', 'SubStateCapture'));
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