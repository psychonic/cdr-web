<?php echo $this->Paginator->counter(); ?> <br />
<table>
	<?php 
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
												$format->applink($html, $app_data['app_id'], $app_data['name']), //$html->link($app_data['name'], array('controller' => 'applications', 'action' => 'view', 'id' => $app_data['app_id'])),
												$app_data['current_version_id']
											)
									),
									null,
									array('class' => 'alt')
								);
		}
	?>
</table> 

<?php echo $this->Paginator->counter(); ?> <br />
<?php echo $this->Paginator->numbers(array('modulus' => null)); ?> <br />
<?php echo $this->Paginator->prev('Previous', null, null); ?> &nbsp;
<?php echo $this->Paginator->next('Next', null, null); ?> 