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
		$this->ContentRecord->unbindModel(array('hasMany' => array('AppStateCapture', 'SubStateCapture')), false);
		
		$data = $this->paginate('ContentRecord');
		
		$this->set('data', $data);
	}
	
	
	function view($id = null) {
		$this->ContentRecord->unbindModel(array('hasMany' => array('AppStateCapture', 'SubStateCapture')), false);
		
		$data = $this->ContentRecord->read();
		
		var_dump($data); 
		$nextCDR = $this->ContentRecord->find('first', array('fields' => array('cdr_id'), 'conditions' => array('cdr_id <' => $id), 'order' => array('cdr_id DESC')));
		
		if($nextCDR == null)
			$nextCDR = '0';
		else
			$nextCDR = $nextCDR['ContentRecord']['cdr_id'];
		
		$appstate_created = $this->ContentRecord->AppStateCapture->find('all', array('conditions' => array('cdr_id' => $nextCDR, 'created' => 1), 'fields' => array('app_id', 'created', 'name'), 'order' => array('app_id ASC')));
		$appstate_modified = $this->ContentRecord->AppStateCapture->find('all', array('conditions' => array('cdr_id' => $nextCDR, 'created' => 0), 'fields' => array('app_id', 'created', 'name'), 'order' => array('app_id ASC')));
		
		$substate_created = $this->ContentRecord->SubStateCapture->find('all', array('conditions' => array('cdr_id' => $nextCDR,  'created' => 1), 'fields' => array('sub_id', 'created', 'name'), 'order' => array('sub_id ASC')));
		$substate_modified = $this->ContentRecord->SubStateCapture->find('all', array('conditions' => array('cdr_id' => $nextCDR,  'created' => 0), 'fields' => array('sub_id', 'created', 'name'), 'order' => array('sub_id ASC')));
		
		$this->set('data', $data);
		$this->set('appstate_created', $appstate_created);
		$this->set('appstate_modified', $appstate_modified);
		$this->set('substate_created', $substate_created);
		$this->set('substate_modified', $substate_modified);
		
		$this->set('title_for_layout', 'CDR - ' . $data['ContentRecord']['cdr_id']);
	}
}
?>