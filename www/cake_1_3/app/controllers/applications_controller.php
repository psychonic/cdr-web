<?php

class ApplicationsController extends AppController
{
	var $name = 'Applications';
	
	var $helpers = array('paginator', 'format');
	
	var $paginate = array(
			'limit' => 100,
			'order' => array(
				'Application.app_id' => 'asc'
			)
		);
	
	function index() {
		$this->Application->unbindModel(array('hasAndBelongsToMany' => array('Subscription')), false); // don't grab expensive assocation
		$this->Application->unbindModel(array('hasMany' => array('AppFilesystem', 'AppVersion','AppStateCapture')), false);
		
		$this->set('data', $this->paginate('Application'));
	}
	
	
	function view($id = null) {
		$this->Application->unbindModel(array('hasAndBelongsToMany' => array('Subscription'))); // don't grab expensive assocation
		$this->Application->unbindModel(array('hasMany' => array('AppStateCapture')));
		
		$wantVersions = isset($this->passedArgs['show_version']) && $this->passedArgs['show_version'] == true;
		
		if(!$wantVersions) {
			$this->Application->unbindModel(array('hasMany' => array('AppVersion')));
		}
		
		$this->Application->app_id = $id;
		$data = $this->Application->read();
		
		// make this a behavior
		$data['LaunchOptions'] = json_decode($data['Application']['launch_options'], true);
		$data['UserDefined'] = json_decode($data['Application']['user_defined'], true);
	
		if(isset($data['AppVersion'])) {
			foreach($data['AppVersion'] as $key => $version) {
				$data['AppVersion'][$key]['launch_option_ids'] = implode(', ', json_decode($version['launch_option_ids']));
			}
		}
		
		$this->set('data', $data);
		$this->set('show_version', $wantVersions);
		
		$this->set('title_for_layout', $data['Application']['app_id'] . ' - ' . $data['Application']['name']);
		$this->set('layout_menuitems', array('view', 'subs', 'hist'));
		$this->set('reference_id', $id);
	}
	
	
	function subs($id = null) {
		$this->Application->unbindModel(array('hasAndBelongsToMany' => array('Subscription')), false);
		$this->Application->unbindModel(array('hasMany' => array('AppFilesystem', 'AppVersion','AppStateCapture')), false);
		
		$this->Application->Subscription->unbindModel(array('hasAndBelongsToMany' => array('Application')), false);
		$this->Application->Subscription->bindModel(array('hasOne'=>array('AppsSubs')), false);
		
		$this->Application->app_id = $id;
		$data = $this->Application->read();

		$this->paginate['order'] = array('Subscription.sub_id' => 'asc');
		$pagination = $this->paginate('Subscription', array('AppsSubs.app_id' => $id));
		
		$this->set('data', $data);
		$this->set('sub_data', $pagination);
		
		$this->set('title_for_layout', $data['Application']['app_id'] . ' - ' . $data['Application']['name']);
		$this->set('layout_menuitems', array('view', 'subs', 'hist'));
		$this->set('reference_id', $id);
	}
	
	
	function hist($id = null) {
		$this->Application->unbindModel(array('hasAndBelongsToMany' => array('Subscription')), false);
		$this->Application->unbindModel(array('hasMany' => array('AppFilesystem', 'AppVersion','AppStateCapture')), false);
		
		$this->Application->app_id = $id;
		$data = $this->Application->read();

		$this->paginate['order'] = array('AppStateCapture.cdr_id' => 'desc');
		
		$hist_data = $this->paginate('AppStateCapture', array('app_id' => $id));

		$hist = $data['Application'];
		$hist_changes = array();
		
		foreach($hist_data as $capture)
		{
			$hcapture = $capture['AppStateCapture'];
			$changed = array();
			
			if($hcapture['created']) {
				$changed[] = 'Created';
			} else {
				foreach($hist as $key => $value) {
				
					if($hcapture[$key] != null && $key != 'app_id') {
						$changed[] = $key . ' = ' . $hcapture[$key];
					}
					
				}
			}
			
			$hist_changes[] = array($hcapture['cdr_id'], $changed);
		}
		
		$this->set('data', $data);
		$this->set('hist_data', $hist_changes);
		
		$this->set('title_for_layout', $data['Application']['app_id'] . ' - ' . $data['Application']['name']);
		$this->set('layout_menuitems', array('view', 'subs', 'hist'));
		$this->set('reference_id', $id);
	}
}

?>