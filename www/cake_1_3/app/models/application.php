<?php

class Application extends AppModel {
	var $name = 'Application';
	
	var $useTable = 'app';
	var $primaryKey = 'app_id';
	var $displayField = 'name';

	var $hasAndBelongsToMany = array(
		'Subscription' =>
			array(
					'className'				=> 'Subscription',
					'joinTable'				=> 'apps_subs',
					'foreign_key'			=> 'app_id',
					'associationForeignKey' => 'sub_id',
					'unique'				=> false,
					'conditions'			=> '',
					'fields'				=> array('sub_id', 'name'),
					'order'					=> '',
					'limit'					=> '',
					'offset'				=> '',
					'finderQuery'			=> '',
					'deleteQuery'			=> '',
					'insertQuery'			=> ''
			)
	);
	
	var $hasMany = array (
		'AppFilesystem' =>
			array (
					'className'				=> 'AppFilesystem',
					'foreignKey'			=> 'app_id',
					'conditions'			=> array('`AppFilesystem`.`cdr_id_last`' => null),
			),
		'AppVersion' =>
			array (
					'className'				=> 'AppVersion',
					'foreignKey'			=> 'app_id',
					'order'					=> 'version_id',
					'conditions'			=> array('`AppVersion`.`cdr_id_last`' => null)
			)
	
	);
}

?>