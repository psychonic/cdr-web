<?php
	    $this->Paginator->options(array('url' => array('id' => $reference_id)));
?>

<table id="info">
	<?php 
		echo $html->tableHeaders(
									array($this->Paginator->sort('App ID', 'app_id'), 
											$this->Paginator->sort('Name', 'name')
										)
							); 
	?>

	<?php

		foreach($app_data as $AppsMany) {
			
			$app = $AppsMany['Application'];
			
			echo $html->tableCells(
									array(
										array($format->columnLiteral($app['app_id']), $format->applink($html, $app['app_id'], $app['name']))
									),
									null,
									array('class' => 'alt')
								);
		}
	?>
</table> 