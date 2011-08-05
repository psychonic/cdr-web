<?php

class ContentRecordsController extends AppController
{
	var $name = 'ContentRecords';
	
	var $helpers = array('paginator', 'format');
	
	var $paginate = array(
			'limit' => 100,
			'order' => array(
				'ContentRecord.cdr_id' => 'desc'
			)
		);
	
	function index() {
		$data = $this->paginate('ContentRecord');
		
		$this->set('data', $data);
	}
	
	
	function view($id = null) {
		$this->ContentRecord->bindCapture();
		
		$data = $this->ContentRecord->read();
		
		$appstate_created = $this->ContentRecord->AppStateCapture->find('all', array('conditions' => array('cdr_id' => $id, 'created' => 1), 'fields' => array('app_id', 'created', 'name'), 'order' => array('app_id ASC')));
		$appstate_modified = $this->ContentRecord->AppStateCapture->find('all', array('conditions' => array('cdr_id' => $id, 'created' => 0), 'fields' => array('app_id', 'created', 'name'), 'order' => array('app_id ASC')));
		
		$appnames = null;

		if(count($appstate_created) + count($appstate_modified) < 2500) {
			$appnames = array();
			$ids = array();
			
			foreach($appstate_created as $app) {
				$ids[] = $app['AppStateCapture']['app_id'];
			}
			
			foreach($appstate_modified as $app) {
				$ids[] = $app['AppStateCapture']['app_id'];
			}
			
			$this->loadModel('Application');
			
			$appdata = $this->Application->find('all', array('fields' => array('app_id','name'), 'conditions' => array('app_id' => $ids)));
			
			foreach($appdata as $app) {
				$appnames[$app['Application']['app_id']] = $app['Application']['name'];
			}
		}
		
		
		$substate_created = $this->ContentRecord->SubStateCapture->find('all', array('conditions' => array('cdr_id' => $id,  'created' => 1), 'fields' => array('sub_id', 'created', 'name'), 'order' => array('sub_id ASC')));
		$substate_modified = $this->ContentRecord->SubStateCapture->find('all', array('conditions' => array('cdr_id' => $id,  'created' => 0), 'fields' => array('sub_id', 'created', 'name'), 'order' => array('sub_id ASC')));
		
		$subnames = null;
		
		if(count($substate_created) + count($substate_modified) < 2000) {
			$subnames = array();
			$ids = array();
			
			foreach($substate_created as $sub) {
				$ids[] = $sub['SubStateCapture']['sub_id'];
			}
			
			foreach($substate_modified as $sub) {
				$ids[] = $sub['SubStateCapture']['sub_id'];
			}
			
			$this->loadModel('Subscription');
			
			$subdata = $this->Subscription->find('all', array('fields' => array('sub_id','name'), 'conditions' => array('sub_id' => $ids)));
			
			foreach($subdata as $sub) {
				$subnames[$sub['Subscription']['sub_id']] = $sub['Subscription']['name'];
			}
		}
		
		$this->set(compact('data', 'appstate_created', 'appstate_modified', 'substate_created', 'substate_modified', 'appnames', 'subnames'));
		
		$this->set('title_for_layout', 'CDR - ' . $data['ContentRecord']['cdr_id']);
	}
}
?>