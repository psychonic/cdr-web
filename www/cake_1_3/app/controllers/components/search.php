<?php

class SearchComponent extends Object {
	var $definition;
	
	function initialize(&$controller, $settings = array()) {
		$this->controller =& $controller;
		$this->definition = $controller->{$controller->modelView}->getSearchableDefinition();
	}
	
	function buildConditions($data) {
		foreach($this->definition as $key => $type) {
			if(isset($data['Search.' . $key])) {
				$passedValue = $data['Search.' . $key];
				
				if($type == "string") {
					$this->controller->paginate['conditions'][][$key . ' LIKE'] = '%' . $passedValue . '%';
				} else {
					$this->controller->paginate['conditions'][][$key] = $passedValue;
				}	
			}
		}
	}
	
	
	function processRedirect($data) {
		foreach($this->definition as $key => $type) {
			if(isset($data[$this->controller->modelView][$key]) && (is_numeric($data[$this->controller->modelView][$key]) || !empty($data[$this->controller->modelView][$key]))) {
				$url['action'] = 'index';
				$url['Search.' . $key] = $data[$this->controller->modelView][$key];
					
				$this->controller->redirect($url, null, true);
				break;
			}
		}
	}
	
}
?>