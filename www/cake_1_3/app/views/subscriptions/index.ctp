<?php echo $this->Paginator->counter(); ?> <br />
<table>
	<?php 
		echo $html->tableHeaders(
									array($this->Paginator->sort('Sub ID', 'sub_id'), 
											$this->Paginator->sort('Name', 'name'),
											//__('App Count', true)
										)
							); 
	?>

	<?php foreach($data as $sub): ?> 
	
	<?php 
		$sub_data = $sub['Subscription'];
		
		echo $html->tableCells(
								array(
									array($sub_data['sub_id'],
											$format->sublink($html, $sub_data['sub_id'], $sub_data['name']),
											//count($sub['Application'])
										)
								),
								null,
								array('class' => 'alt')
							);
		
	?>
	
	<?php endforeach; ?> 
</table> 

<?php echo $this->Paginator->counter(); ?> <br />
<?php echo $this->Paginator->numbers(array('modulus' => null)); ?> <br />
<?php echo $this->Paginator->prev('Previous', null, null); ?> &nbsp;
<?php echo $this->Paginator->next('Next', null, null); ?> 