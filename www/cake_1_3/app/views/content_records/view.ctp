<table id="info">
	<?php
		$cdr_info = $data['ContentRecord'];
		
		$appAddBuffer = '';
		$appChangeBuffer = '';
		$subAddBuffer = '';
		$subChangeBuffer = '';
		
		function bufferApps(&$format, &$html, &$apps, &$buffer) {
			$c = count($apps);
			
			if($c == 0) {
				$buffer = __('None', true);
				return;
			}
			
			$linkTemplate = $format->applink($html, '0');
			
			foreach($apps as $app) {
				$app_info =& $app['AppStateCapture'];
				$buffer .= str_replace('0', $app_info['app_id'], $linkTemplate);
				if($c-- > 1) $buffer .= ', ';
			}
		}
		
		function bufferSubs(&$format, &$html, &$subs, &$buffer) {
			$c = count($subs);
			
			if($c == 0) {
				$buffer = __('None', true);
				return;
			}
			
			$linkTemplate = $format->sublink($html, '0');
			
			foreach($subs as $sub) {
				$sub_info =& $sub['SubStateCapture'];
				$buffer .= str_replace('0', $sub_info['sub_id'], $linkTemplate);
				if($c-- > 1) $buffer .= ', ';
			}
		}
		
		bufferApps($format, $html, $appstate_created, $appAddBuffer);
		bufferApps($format, $html, $appstate_modified, $appChangeBuffer);
		bufferSubs($format, $html, $substate_created, $subAddBuffer );
		bufferSubs($format, $html, $substate_modified, $subChangeBuffer );
		
		echo $html->tableCells(
						array(
							array( $format->column('CDR ID'), $cdr_info['cdr_id'] ),
							array( $format->column('Hash'), $cdr_info['hash'] ),
							array( $format->column('Date Updated'), $cdr_info['date_updated'] ),
							array( $format->column('Date Processed'), $cdr_info['date_processed'] ),
							array( $format->column('Apps Added'), $appAddBuffer ),
							array( $format->column('Apps Modified'), $appChangeBuffer ),
							array( $format->column('Subs Added'), $subAddBuffer ),
							array( $format->column('Subs Modified'), $subChangeBuffer ),
						),
						null,
						array('class' => 'alt')
					);
	?>
</table>