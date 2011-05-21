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
		$this->set('data', $this->paginate('Subscription'));
	}
}

?>