<?php
	    $this->Paginator->options(array('url' => array('id' => $reference_id)));
?>

<table id="info">
	<?php 
		echo $html->tableHeaders(
									array($this->Paginator->sort('Sub ID', 'sub_id'), 
											$this->Paginator->sort('Name', 'name')
										)
							); 
	?>

	<?php

		foreach($sub_data as $SubsMany) {
			
			$sub = $SubsMany['Subscription'];
			
			echo $html->tableCells(
									array(
										array($format->columnLiteral($sub['sub_id']), $format->sublink($sub['sub_id'], $sub['name']))
									),
									null,
									array('class' => 'alt')
								);
		}
	?>
</table> 