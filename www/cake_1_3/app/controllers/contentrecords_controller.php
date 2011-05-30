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

		$nextCDR = $this->ContentRecord->bottom($id);
		
		if($nextCDR == false)
			$nextCDR = '0';
		
		$appstate_created = $this->ContentRecord->AppStateCapture->find('all', array('conditions' => array('cdr_id' => $nextCDR, 'created' => 1), 'fields' => array('app_id', 'created', 'name'), 'order' => array('app_id ASC')));
		$appstate_modified = $this->ContentRecord->AppStateCapture->find('all', array('conditions' => array('cdr_id' => $nextCDR, 'created' => 0), 'fields' => array('app_id', 'created', 'name'), 'order' => array('app_id ASC')));
		
		$appnames = null;
		
		if(count($appstate_created) + count($appstate_modified) < 500) {
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
		
		
		$substate_created = $this->ContentRecord->SubStateCapture->find('all', array('conditions' => array('cdr_id' => $nextCDR,  'created' => 1), 'fields' => array('sub_id', 'created', 'name'), 'order' => array('sub_id ASC')));
		$substate_modified = $this->ContentRecord->SubStateCapture->find('all', array('conditions' => array('cdr_id' => $nextCDR,  'created' => 0), 'fields' => array('sub_id', 'created', 'name'), 'order' => array('sub_id ASC')));
		
		$subnames = null;
		
		if(count($substate_created) + count($substate_modified) < 500) {
			$subnames = array();
			$ids = array();
			
			foreach($substate_created as $app) {
				$ids[] = $app['SubStateCapture']['sub_id'];
			}
			foreach($substate_modified as $app) {
				$ids[] = $app['SubStateCapture']['sub_id'];
			}
			
			$this->loadModel('Subscription');
			
			$subdata = $this->Subscription->find('all', array('fields' => array('sub_id','name'), 'conditions' => array('sub_id' => $ids)));
			
			foreach($subdata as $sub) {
				$subnames[$sub['Subscription']['sub_id']] = $sub['Subscription']['name'];
			}
		}
		
		$this->set('data', $data);
		$this->set('appstate_created', $appstate_created);
		$this->set('appstate_modified', $appstate_modified);
		$this->set('substate_created', $substate_created);
		$this->set('substate_modified', $substate_modified);
		
		$this->set('appnames', $appnames);
		$this->set('subnames', $subnames);
		
		$this->set('title_for_layout', 'CDR - ' . $data['ContentRecord']['cdr_id']);
	}
}
?>