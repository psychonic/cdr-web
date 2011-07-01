 <?php
 
class HistoryComponent extends Object {

	function initialize(&$controller, $settings = array()) {
		$this->controller =& $controller;
	}

	// this is done in reverse to get the oldest value for the cdr we want
	// we only want a single row, modified with the capture state (low -> high, gives us old -> new)
	function incrementalBuildState(&$current_state, $capture_state) {
		foreach($current_state[$this->controller->modelView] as $key => $value)
		{
			foreach($capture_state as $hist_data) {
				$hcapture = $hist_data[$this->controller->modelCapture];
				
				if($key != 'date_updated' && $hcapture[$key] != NULL && $current_state[$this->controller->modelView][$key] != $hcapture[$key]) {
					$current_state[$this->controller->modelView][$key] = $hcapture[$key];
					break;
				}
			}
		}
	}
	
	// of the columns that changed in a CDR, grab the values from a newer CDR and display the newer CDR, this would show newest values
	// iterate through, build historical state, display the columns that changed in the (next, older) CDR
	// high -> low
	function buildHistoricalChanges(&$current_data, $capture_data) {
		$hist = $current_data[$this->controller->modelView];
		$hist_changes = array();
		
		$this->controller->LoadModel('ContentRecord');

		for($i = 0; $i < count($capture_data); $i++) {
			$changed = array();
			
			$hcapture = $capture_data[$i][$this->controller->modelCapture];
			$cdr_id = $this->controller->ContentRecord->top($hcapture['cdr_id']); // just make this +1?

			if($i >= 0 && $hcapture['created'] == true) {
				$hist_changes[] = array($cdr_id, array('Created'));
			} else {
				// this is a slightly different loop than incrementalBuildState, we want to iterate all the changed columns in a row before we move on
				foreach($hist as $key => $value) {
					if($key != 'date_updated' && $key != $this->controller->modelPK && $hcapture[$key] != NULL && $hist[$key] != $hcapture[$key] ) {
						$changed[] = $key . ' => ' . $hist[$key];
						$hist[$key] = $hcapture[$key];
					}
				}
				
				$hist_changes[] = array($cdr_id, $changed);
			}
		}
		
		return $hist_changes;
	}

}

?>