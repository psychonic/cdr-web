<?php

class ApplicationsController extends AppController
{
	var $name = 'Applications';
	
	var $modelView = 'Application';
	var $modelCapture = 'AppStateCapture';
	
	var $helpers = array('paginator', 'format');
	
	var $paginate = array(
			'limit' => 100,
			'order' => array(
				'Application.app_id' => 'asc'
			)
		);
	
	function index() {
		$this->set('data', $this->paginate($this->modelView));
	}
	
	
	function view($id = null) {
		$wantVersions = isset($this->passedArgs['show_version']) && $this->passedArgs['show_version'] == true;
		
		$cdr_want = null;
			
		if(isset($this->passedArgs['cdr_id'])) {
			$cdr_want = $this->passedArgs['cdr_id'];
			
			$this->Application->cdr_target = $cdr_want;
			$this->Application->bindCapture();
		}
		
		$this->Application->bindImmediate($wantVersions);
		
		$this->Application->app_id = $id;
		$data = $this->Application->read();

		// build data to fit old CDR if set
		// this is done in reverse to get the oldest value for the cdr we want, compared to the history page which is a descending list of all captures
		
		if(isset($cdr_want)) {
			$history = $this->Application->findCapture();

			foreach($data['Application'] as $key => $value)
			{
				foreach($history as $hist_data) {
					$hcapture = $hist_data['AppStateCapture'];
					
					if($hcapture[$key] != NULL && $data['Application'][$key] != $hcapture[$key]) {
						$data['Application'][$key] = $hcapture[$key];
						break;
					}
				}
			}
		}
		
		$this->Application->expand($data);
		
		$this->set('data', $data);
		$this->set('show_version', $wantVersions);
		
		$this->set('title_for_layout', $data['Application']['app_id'] . ' - ' . $data['Application']['name']);
		$this->set('layout_menuitems', array('view', 'subs', 'history' => 'hist'));
		$this->set('reference_id', $id);
		$this->set('reference_cdr', $cdr_want);
	}
	
	
	function subs($id = null) {
		$this->Application->bindMany();
		
		$this->Application->app_id = $id;
		$data = $this->Application->read();

		$this->paginate['order'] = array('Subscription.sub_id' => 'asc');
		$pagination = $this->paginate('Subscription', array('AppsSubs.app_id' => $id));
		
		$this->set('data', $data);
		$this->set('sub_data', $pagination);
		
		$this->set('title_for_layout', $data['Application']['app_id'] . ' - ' . $data['Application']['name']);
		$this->set('layout_menuitems', array('view', 'subs', 'history' => 'hist'));
		$this->set('reference_id', $id);
	}
	
	
	function hist($id = null) {
		$this->Application->bindCapture();
		
		$this->Application->app_id = $id;
		$data = $this->Application->read();

		$this->paginate['order'] = array('AppStateCapture.cdr_id' => 'desc');
		
		$hist_data = $this->paginate('AppStateCapture', array('app_id' => $id));

		$hist = $data['Application'];
		$hist_changes = array();
		
		$this->LoadModel('ContentRecord');
		$topCDR = $this->ContentRecord->top($hist_data[0]['AppStateCapture']['cdr_id']);
		
		// of the columns that changed in a CDR, grab the values from a newer CDR and display the newer CDR, this would show newest values
		// iterate through, build historical state, display the columns that changed in the (next, older) CDR
		for($i = -1; $i < count($hist_data) - 1; $i++) {
			$changed = array();
			$hcapture = null;
			$cdr_id = null;
			
			if($i == -1) {
				$hcapture = $hist;
				$cdr_id = $topCDR;
			} else {
				$hcapture = $hist_data[($i < 0 ? 0 : $i)]['AppStateCapture'];
				$cdr_id = $hcapture['cdr_id'];
			}
			
			$ncapture = $hist_data[$i+1]['AppStateCapture'];
			
			if($ncapture['created'] == true) {
				$changed[] = 'Created';
			} else {
				foreach($hist as $key => $value) {
					if($ncapture[$key] != null && $key != 'app_id') {
						$changed[] = $key . ' => ' . $hist[$key];
						$hist[$key] = $ncapture[$key];
					}
				}
			}
			$hist_changes[] = array($cdr_id, $changed);
		}
		
		$this->set('data', $data);
		$this->set('hist_data', $hist_changes);
		
		$this->set('title_for_layout', $data['Application']['app_id'] . ' - ' . $data['Application']['name']);
		$this->set('layout_menuitems', array('view', 'subs', 'history' => 'hist'));
		$this->set('reference_id', $id);
	}
}

?>