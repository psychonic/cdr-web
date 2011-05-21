<?php
	    $this->Paginator->options(array('url' => $this->passedArgs));
?>

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
										array($format->columnLiteral($format->applink($html, $reference_id, $changes[0], array('cdr_id'=>$changes[0]))),
												implode(', ',$changes[1]))
									),
									null,
									array('class' => 'alt')
								);
		}
	?>
</table> 

<?php echo $this->Paginator->counter(); ?> <br />
<?php echo $this->Paginator->numbers(); ?> <br />
<?php echo $this->Paginator->prev('Previous', null, null); ?> &nbsp;
<?php echo $this->Paginator->next('Next', null, null); ?> 