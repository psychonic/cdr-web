<table>
	<?php 
		echo $html->tableHeaders(
									array($this->Paginator->sort('Sub ID', 'sub_id'), 
											$this->Paginator->sort('Name', 'name')
										)
							); 
	?>

	<?php
		foreach($data as $sub) {

			$sub_data = $sub['Subscription'];
		
			echo $html->tableCells(
									array(
										array($sub_data['sub_id'],
											$format->sublink($html, $sub_data['sub_id'], $sub_data['name'])
										)
									),
									null,
									array('class' => 'alt')
								);
		}
	?>
</table> 