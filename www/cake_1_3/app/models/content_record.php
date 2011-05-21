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
}

?>