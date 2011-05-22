<?php

class SearchBehavior extends ModelBehavior {

		function getSearchableDefinition(&$model) {
			$definition = array();
			$schema = $model->schema();
		
			foreach($schema as $field => $field_data) {
				$definition[$field] = $field_data['type'];
			}	

			return $definition;
	}
	
}

?>