<table id="info">
	<?php
		$sub_info = $data['Subscription'];
		
		// optimize this?
		$userBuffer = '';
		$userSubLinks = array('OnPurchaseGrantGuestPassPackage', 'OnPurchaseGrantGuestPassPackage1', 'OnPurchaseGrantGuestPassPackage2', 'OnPurchaseGrantGuestPassPackage3', 'OnPurchaseGrantGuestPassPackage4');
		$userAppLinks = array('AppIDOwnedRequired');
		
		foreach($data['ExtendedInfo'] as $key => $value) {
			if(in_array($key, $userSubLinks)) {
				$value = $format->sublink($value);
			} else if(in_array($key, $userAppLinks)) {
				$value = $format->applink($value);
			}
			
			if(substr($value,0,7) == 'http://') {
				$value = '<a href="' . $value . '">' . $value . '</a>';
			}
			
			$userBuffer .= $key . ' = ' . $value . '<br />';
		}
		
		echo $html->tableCells(
							array(
								array( $format->column('Sub ID'), $sub_info['sub_id'] ),
								array( $format->column('Name'), $sub_info['name'] ),
								array( $format->column('Billing Type'), $sub_info['billing_type'] ),
								array( $format->column('Cost (in cents)'), $sub_info['cost_in_cents'] ),
								array( $format->column('Period (in minutes)'), $sub_info['period_in_minutes'] ),
								array( $format->column('On Subscribe Run App ID'), $format->applink($sub_info['on_subscribe_run_app_id']) ),
								array( $format->column('On Subscribe Launch Option'), $sub_info['on_subscribe_run_launch_option_index'] ),
								array( $format->column('Is Preorder'), $format->boolstring($sub_info['is_preorder']) ),
								array( $format->column('Requires Shipping Address'), $format->boolstring($sub_info['requires_shipping_address']) ),
								array( $format->column('Domestic Cost'), $sub_info['domestic_cost_in_cents'] ),
								array( $format->column('International Cost'), $sub_info['international_cost_in_cents'] ),
								array( $format->column('Required Key Type'), $sub_info['required_key_type'] ),
								array( $format->column('Is Cyber Cafe'), $format->boolstring($sub_info['is_cyber_cafe']) ),
								array( $format->column('Game Code'), $sub_info['game_code'] ),
								array( $format->column('Game Code Description'), $sub_info['game_code_description'] ),
								array( $format->column('Is Disabled'), $format->boolstring($sub_info['is_disabled']) ),
								array( $format->column('Requires CD'), $format->boolstring($sub_info['requires_cd']) ),
								array( $format->column('Territory Code'), $sub_info['territory_code'] ),
								array( $format->column('Is Steam3 Subscription'), $format->boolstring($sub_info['is_steam3_subscription']) ),
								array( $format->column('Extended Info'),  $userBuffer )
								
							),
							null,
							array('class' => 'alt')
						);
	?>
</table>