<?php

class ContentRecord extends AppModel {
	var $name = 'ContentRecord';
	
	var $useTable = 'cdr';
	var $primaryKey = 'cdr_id';
	var $displayField = 'cdr_id';
	
	var $hasMany = array(
		'AppStateCapture' =>
			array (
					'className'			=> 'AppStateCapture',
					'foreignKey'			=> 'cdr_id',
					'order'					=> 'app_id'
			),
		'SubStateCapture' =>
			array (
					'className'			=> 'SubStateCapture',
					'foreignKey'			=> 'cdr_id',
					'order'					=> 'sub_id'
			)
		);
		
	function top($id) {
		return $this->field('cdr_id', array('cdr_id >' => $id), 'cdr_id ASC');
	}
	
	function bottom($id) {
		return $this->field('cdr_id', array('cdr_id <' => $id), 'cdr_id DESC');
	}
}

?>