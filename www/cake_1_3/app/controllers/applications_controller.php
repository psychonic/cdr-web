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
		$this->Application->unbindModel(array('hasMany' => array('AppFilesystem', 'AppVersion')), false);
		
		$this->set('data', $this->paginate('Application'));
	}
	
	function view($id = null) {
		$this->Application->unbindModel(array('hasAndBelongsToMany' => array('Subscription'))); // don't grab expensive assocation
	
		$wantVersions = isset($this->passedArgs['show_version']) && $this->passedArgs['show_version'] == true;
		
		if(!$wantVersions) {
			$this->Application->unbindModel(array('hasMany' => array('AppVersion')));
		}
		
		$this->Application->app_id = $id;
		$data = $this->Application->read();

		$this->set('title_for_layout', $data['Application']['app_id'] . ' - ' . $data['Application']['name']);
		
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
		
		$this->set('layout_menuitems', array('view' => 'view', 'subs' => 'subs', 'history' => 'hist'));
		$this->set('reference_id', $id);
	}
	
	function subs($id = null) {
		$this->Application->unbindModel(array('hasMany' => array('AppFilesystem', 'AppVersion')), false);
		
		$this->Application->app_id = $id;
		$data = $this->Application->read();
		
		$this->set('title_for_layout', $data['Application']['app_id'] . ' - ' . $data['Application']['name']);
		$this->set('data', $data);
		
		$this->set('layout_menuitems', array('view' => 'view', 'subs' => 'subs', 'history' => 'hist'));
		$this->set('reference_id', $id);
	}
}

?>