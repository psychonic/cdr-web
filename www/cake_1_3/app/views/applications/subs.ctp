<?php
	    $this->Paginator->options(array('url' => $this->passedArgs));
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
										array($format->columnLiteral($sub['sub_id']), $format->sublink($html, $sub['sub_id'], $sub['name']))
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