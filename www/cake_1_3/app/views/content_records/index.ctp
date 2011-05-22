<table>
	<?php 
		echo $html->tableHeaders(
									array($this->Paginator->sort('CDR ID', 'cdr_id'), 
											$this->Paginator->sort('Date Updated', 'date_updated'),
											$this->Paginator->sort('Date Processed', 'date_processed'),
											$this->Paginator->sort('App Count', 'app_count'),
											$this->Paginator->sort('Sub Count', 'sub_count'),
										)
							); 
	?>

	<?php
		foreach($data as $cdr) {

			$cdr_data = $cdr['ContentRecord'];
			
			echo $html->tableCells(
									array(
										array($format->cdrlink($cdr_data['cdr_id']),
													$cdr_data['date_updated'],
													$cdr_data['date_processed'],
													$cdr_data['app_count'],
													$cdr_data['sub_count']
											)
									),
									null,
									array('class' => 'alt')
								);
		}
	?>
</table> 