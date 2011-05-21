<?php

class Subscription extends AppModel {
	var $name = 'Subscription';
	
	var $useTable = 'sub';
	var $primaryKey = 'sub_id';
	var $displayField = 'name';
	
	var $hasAndBelongsToMany = array(
		'Application' =>
			array(
					'className'				=> 'Application',
					'joinTable'				=> 'apps_subs',
					'foreign_key'			=> 'sub_id',
					'associationForeignKey' => 'app_id',
					'unique'				=> false,
					'conditions'			=> '',
					'fields'				=> array('app_id', 'name'),
					'order'					=> '',
					'limit'					=> '',
					'offset'				=> '',
					'finderQuery'			=> '',
					'deleteQuery'			=> '',
					'insertQuery'			=> ''
			)
	);
	
	var $hasMany = array (
		'SubStateCapture' =>
			array (
					'className'			=> 'SubStateCapture',
					'foreignKey'			=> 'sub_id',
					'order'					=> 'cdr_id'
			)
	);
}

?>