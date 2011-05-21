<table id="info">
	<?php 
		echo $html->tableHeaders(
									array('Sub ID', 
											'Name'
										)
							); 
	?>

	<?php
		foreach($data['Subscription'] as $sub_data) {
		
			echo $html->tableCells(
									array(
										array($format->columnLiteral($sub_data['sub_id']), $format->sublink($html, $sub_data['sub_id'], $sub_data['name']))
									),
									null,
									array('class' => 'alt')
								);
		}
	?>
</table> 