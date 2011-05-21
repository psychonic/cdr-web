<?php

class SubscriptionsController extends AppController
{
	var $name = 'Subscriptions';
	
	var $helpers = array('paginator', 'format');
	
	var $paginate = array(
			'limit' => 100,
			'order' => array(
				'Subscription.sub_id' => 'asc'
			)
		);
	
	function index() {
		$this->Subscription->unbindModel(array('hasAndBelongsToMany' => array('Application')), false); // don't grab expensive assocation
		$this->Subscription->unbindModel(array('hasMany' => array('SubStateCapture')));
				
		$this->set('data', $this->paginate('Subscription'));
	}
	
	function view($id = null) {
		$this->Subscription->unbindModel(array('hasAndBelongsToMany' => array('Application'))); // don't grab expensive assocation
		$this->Subscription->unbindModel(array('hasMany' => array('SubStateCapture')));
		
		
		$this->Subscription->sub_id = $id;
		$data = $this->Subscription->read();
		
		
		// build data to fit old CDR if set
		// this is done in reverse to get the oldest value for the cdr we want, compared to the history page which is a descending list of all captures
		$cdr_want = null;
		
		if(isset($this->passedArgs['cdr_id'])) {
			$cdr_want = $this->passedArgs['cdr_id'];
			
			$history = $this->Subscription->SubStateCapture->find('all', array('conditions' => array('sub_id' => $id, 'cdr_id >=' => $cdr_want), 'order' => 'cdr_id ASC'));

			foreach($data['Subscription'] as $key => $value)
			{
				foreach($history as $hist_data) {
					$hcapture = $hist_data['AppStateCapture'];
					
					if($hcapture[$key] != NULL && $data['Subscription'][$key] != $hcapture[$key]) {
						$data['Subscription'][$key] = $hcapture[$key];
						break;
					}
				}
			}
		}
		
		$data['ExtendedInfo'] = json_decode($data['Subscription']['extended_info']);
		
		$this->set('data', $data);
		
		$this->set('title_for_layout', $data['Subscription']['sub_id'] . ' - ' . $data['Subscription']['name']);
		$this->set('layout_menuitems', array('view', 'apps', 'history' => 'hist'));
		$this->set('reference_id', $id);
		$this->set('reference_cdr', $cdr_want);
	}
		
}

?>