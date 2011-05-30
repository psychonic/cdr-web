<table>
	<?php 
		$this->Paginator->options(array('url' => $this->passedArgs));
	
		echo $html->tableHeaders(
									array($this->Paginator->sort('App ID', 'app_id'), 
											$this->Paginator->sort('Name', 'name'),
											$this->Paginator->sort('Version', 'current_version_id')
										)
							); 
	?>

	<?php
		foreach($data as $app) {

			$app_data = $app['Application'];
		
			echo $html->tableCells(
									array(
										array($app_data['app_id'],
												$format->applink($app_data['app_id'], $app_data['name']),
												$app_data['current_version_id']
											)
									),
									null,
									array('class' => 'alt')
								);
		}
	?>
</table> 