<table id="info">
	<?php
		$cdr_info = $data['ContentRecord'];
		
		$appAddBuffer = '';
		$appChangeBuffer = '';
		$subAddBuffer = '';
		$subChangeBuffer = '';
		
		function bufferApps(&$format, &$html, &$apps, &$names, &$buffer) {
			$c = count($apps);
			
			if($c == 0) {
				$buffer = __('None', true);
				return;
			}
			
			$linkTemplate = str_replace('0', '%s', $format->applink('0'));
			
			if(isset($names)) $buffer .= '<table class="subcontent">';
			
			foreach($apps as $app) {
				$app_info =& $app['AppStateCapture'];

				if(isset($names)) {
						$buffer .= $html->tableCells(
								array(
									array(sprintf($linkTemplate, $app_info['app_id'], $app_info['app_id']), sprintf($linkTemplate, $app_info['app_id'], $names[$app_info['app_id']]))
								),
								array('class' => 'even'),
								array('class' => 'odd')
							);
				} else {
					$buffer .= sprintf($linkTemplate, $app_info['app_id'], $app_info['app_id']);
					if($c-- > 1) $buffer .= ',&nbsp; &nbsp;';
				}
			}
			
			if(isset($names)) $buffer .= '</table>';
		}
		
		function bufferSubs(&$format, &$html, &$subs, &$names, &$buffer) {
			$c = count($subs);
			
			if($c == 0) {
				$buffer = __('None', true);
				return;
			}
			
			$linkTemplate = str_replace('0', '%s', $format->sublink('0'));
			
			if(isset($names)) $buffer .= '<table class="subcontent">';
			
			foreach($subs as $sub) {
				$sub_info =& $sub['SubStateCapture'];
				
				if(isset($names)) {
						$buffer .= $html->tableCells(
								array(
									array(sprintf($linkTemplate, $sub_info['sub_id'], $sub_info['sub_id']), sprintf($linkTemplate, $sub_info['sub_id'], $names[$sub_info['sub_id']]))
								),
								array('class' => 'even'),
								array('class' => 'odd')
							);
				} else {
					$buffer .= sprintf($linkTemplate, $sub_info['sub_id'], $sub_info['sub_id']);
					if($c-- > 1) $buffer .= ',&nbsp; &nbsp;';
				}
			}

			if(isset($names)) $buffer .= '</table>';
		}
		
		bufferApps($format, $html, $appstate_created, $appnames, $appAddBuffer);
		bufferApps($format, $html, $appstate_modified, $appnames, $appChangeBuffer);
		bufferSubs($format, $html, $substate_created, $subnames, $subAddBuffer );
		bufferSubs($format, $html, $substate_modified, $subnames, $subChangeBuffer );
		
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