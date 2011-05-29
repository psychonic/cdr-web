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
				
				if($hcapture[$key] != NULL && $current_state[$this->controller->modelView][$key] != $hcapture[$key]) {
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
		$topCDR = $this->controller->ContentRecord->top($capture_data[0][$this->controller->modelCapture]['cdr_id']);
		

		for($i = -1; $i < count($capture_data) - 1; $i++) {
			$changed = array();
			$hcapture = null;
			$cdr_id = null;
			
			if($i == -1) {
				$hcapture = $hist;
				$cdr_id = $topCDR;
			} else {
				$hcapture = $capture_data[($i < 0 ? 0 : $i)][$this->controller->modelCapture];
				$cdr_id = $hcapture['cdr_id'];
			}
			
			$ncapture = $capture_data[$i+1][$this->controller->modelCapture];
			
			if($ncapture['created'] == true) {
				$changed[] = 'Created';
			} else {
				// this is a slightly different loop than incrementalBuildState, we want to iterate all the changed columns in a row before we move on
				foreach($hist as $key => $value) {
					if($ncapture[$key] != null && $key != $this->controller->modelPK) {
						$changed[] = $key . ' => ' . $hist[$key];
						$hist[$key] = $ncapture[$key];
					}
				}
			}
			$hist_changes[] = array($cdr_id, $changed);
		}
		
		return $hist_changes;
	}
}

?>