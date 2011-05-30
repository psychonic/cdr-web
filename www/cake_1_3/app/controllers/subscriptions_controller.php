<?php

class SubscriptionsController extends AppController
{
	var $name = 'Subscriptions';
	
	var $modelView = 'Subscription';
	var $modelCapture = 'SubStateCapture';
	var $modelPK = 'sub_id';
		
	var $helpers = array('paginator', 'format');
	var $components = array('History', 'Search');
	
	var $paginate = array(
			'limit' => 100,
			'order' => array(
				'Subscription.sub_id' => 'asc'
			)
		);
	
	function index() {
		$this->Search->buildConditions($this->passedArgs);
		
		$this->set('data', $this->paginate($this->modelView));
	}
	
	
	function view($id = null) {
		$cdr_want = null;
			
		if(isset($this->passedArgs['cdr_id'])) {
			$cdr_want = $this->passedArgs['cdr_id'];
			
			$this->Subscription->cdr_target = $cdr_want;
			$this->Subscription->bindCapture();
		}

		$this->Subscription->sub_id = $id;
		$data = $this->Subscription->read();

		if(isset($cdr_want)) {
			$history = $this->Subscription->findCapture();

			$this->History->incrementalBuildState($data, $history);
		}

		$this->Subscription->expand($data);
		
		$this->set('data', $data);
		
		$this->set('title_for_layout', $data['Subscription']['sub_id'] . ' - ' . $data['Subscription']['name']);
		$this->set('layout_menuitems', array('view', 'apps', 'history' => 'hist'));
		$this->set('reference_id', $id);
		$this->set('reference_cdr', $cdr_want);
	}


	function apps($id = null) {
		$this->Subscription->bindMany();
		
		$this->Subscription->sub_id = $id;
		$data = $this->Subscription->read();

		$this->paginate['order'] = array('Application.app_id' => 'asc');
		$pagination = $this->paginate('Application', array('AppsSubs.sub_id' => $id));
		
		$this->set('data', $data);
		$this->set('app_data', $pagination);
		
		$this->set('title_for_layout', $data['Subscription']['sub_id'] . ' - ' . $data['Subscription']['name']);
		$this->set('layout_menuitems', array('view', 'apps', 'history' => 'hist'));
		$this->set('reference_id', $id);
	}


	function hist($id = null) {
		$this->Subscription->bindCapture();
		
		$this->Subscription->sub_id = $id;
		$data = $this->Subscription->read();

		$this->paginate['order'] = array('SubStateCapture.cdr_id' => 'desc');
		
		$hist_data = $this->paginate('SubStateCapture', array('sub_id' => $id));

		$hist_changes = $this->History->buildHistoricalChanges($data, $hist_data);
		
		$this->set('data', $data);
		$this->set('hist_data', $hist_changes);
		
		$this->set('title_for_layout', $data['Subscription']['sub_id'] . ' - ' . $data['Subscription']['name']);
		$this->set('layout_menuitems', array('view', 'apps', 'history' => 'hist'));
		$this->set('reference_id', $id);
	}
	
	function search() {
			if(is_array($this->data)) {
				$this->Search->processRedirect($this->data);
			}
			
			$this->set('title_for_layout', 'Subscription Search');
	}
}

?>