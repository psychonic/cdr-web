<?php

class ApplicationsController extends AppController
{
	var $name = 'Applications';
	
	var $modelView = 'Application';
	var $modelCapture = 'AppStateCapture';
	var $modelPK = 'app_id';
	
	var $helpers = array('paginator', 'format');
	var $components = array('History');
	
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

		if(isset($cdr_want)) {
			$history = $this->Application->findCapture();

			$this->History->incrementalBuildState($data, $history);
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

		$hist_changes = $this->History->buildHistoricalChanges($data, $hist_data);
		
		$this->set('data', $data);
		$this->set('hist_data', $hist_changes);
		
		$this->set('title_for_layout', $data['Application']['app_id'] . ' - ' . $data['Application']['name']);
		$this->set('layout_menuitems', array('view', 'subs', 'history' => 'hist'));
		$this->set('reference_id', $id);
	}
}

?>