<table>
	<?php 
		$this->Paginator->options(array('url' => $this->passedArgs));
		
		echo $html->tableHeaders(
									array($this->Paginator->sort('Sub ID', 'sub_id'), 
											$this->Paginator->sort('Name', 'name'),
											$this->Paginator->sort('Last Updated', 'date_updated')
										)
							); 
	?>

	<?php
		foreach($data as $sub) {

			$sub_data = $sub['Subscription'];
		
			echo $html->tableCells(
									array(
										array($sub_data['sub_id'],
											$format->sublink($sub_data['sub_id'], $sub_data['name']),
											$format->columnLiteral($sub_data['date_updated'], 'columnlite')
										)
									),
									null,
									array('class' => 'alt')
								);
		}
	?>
</table> 