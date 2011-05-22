<table id="info">
	<?php 
		echo $html->tableHeaders(
									array('CDR ID', 
											'Changed State'
										)
							); 
	?>

	<?php

		foreach($hist_data as $changes) {
			
			echo $html->tableCells(
									array(
										array($format->columnLiteral($format->applink($reference_id, $changes[0], array('cdr_id'=>$changes[0]))),
												implode(', ',$changes[1]))
									),
									null,
									array('class' => 'alt')
								);
		}
	?>
</table> 