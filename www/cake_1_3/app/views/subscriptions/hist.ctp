<table id="info">
	<?php 
		echo $html->tableHeaders(
									array('CDR ID', 
											'Previous State'
										)
							); 
	?>

	<?php

		foreach($hist_data as $changes) {
			
			echo $html->tableCells(
									array(
										array($format->columnLiteral($format->sublink($reference_id, $changes[0], array('cdr_id'=>$changes[0]))),
												implode(', ',$format->prettychange($changes[1])))
									),
									null,
									array('class' => 'alt')
								);
		}
	?>
</table> 