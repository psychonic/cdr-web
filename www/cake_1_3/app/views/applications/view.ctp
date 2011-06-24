<table id="info">
	<?php
		$app_info = $data['Application'];
		
		$launchBuffer = '';
		$versionBuffer = '';
		$fsBuffer = '';
		$userBuffer = '';
		$versionLink = '';
		
		foreach($data['LaunchOptions'] as $launch) {
			$launchBuffer .= '<table class="subcontent">';
			
			if(isset($launch['platform']))
				$optional = array($format->column('Platform'), $launch['platform']);
			else
				$optional = array();
				
			$launchBuffer .= $html->tableCells(
											array(
												array($format->column('Description'), $launch['description']),
												array($format->column('Command Line'), $launch['commandLine']),
												array($format->column('Icon Index'), $launch['iconIndex']),
												array($format->column('No Desktop Shortcut'), $format->boolstring($launch['noDesktopShortcut'])),
												array($format->column('No Start Menu Shortcut'), $format->boolstring($launch['noStartMenuShortcut'])),
												array($format->column('Long Running Unattended'), $format->boolstring($launch['longRunningUnattended'])),
												$optional
											),
											array('class' => 'even'),
											array('class' => 'odd')
										);
			$launchBuffer .= '</table>';
		}
		
		if($show_version) {
		
			$versionLink = $format->applink($app_info['app_id'], 'hide', array('cdr_id' => $reference_cdr));
			
			foreach($data['AppVersion'] as $version) {
				$versionBuffer .= '<table class="subcontent">';
				$versionBuffer .= $html->tableCells(
												array(
													array($format->column('Description'), $version['description']),
													array($format->column('Version ID'), $version['version_id']),
													array($format->column('Is Not Available'), $format->boolstring($version['is_not_available'])),
													array($format->column('Launch Option IDs'), $version['launch_option_ids']),
													array($format->column('Depot Key'), $version['depot_key']),
													array($format->column('Is Key Available'), $format->boolstring($version['is_encryption_key_available'])),
													array($format->column('Is Rebased'), $format->boolstring($version['is_rebased'])),
													array($format->column('Is Long Version Roll'), $format->boolstring($version['is_long_version_roll']))
												),
												array('class' => 'even'),
												array('class' => 'odd')
											);
				$versionBuffer .= '</table>';
			}
		
		} else {
			$versionLink = $format->applink($app_info['app_id'], 'show', array('show_version' => 1, 'cdr_id' => $reference_cdr));
			$versionBuffer = 'Hidden';
		}
		
		
		foreach($data['AppFilesystem'] as $fs) {
			$fsBuffer .= '<table class="subcontent">';
			
			$fsTable = 	array(
									array($format->column('App ID'), $format->applink($fs['app_id_filesystem'])),
									array($format->column('Is Optional'), $format->boolstring($fs['is_optional']))
								);
			
			if(!empty($fs['platform']))
				$fsTable[] = array($format->column('Platform'), $fs['platform']);
				
			if(!empty($fs['mount_name']))
				$fsTable[] = array($format->column('Mount Name'), $fs['mount_name']);
				
			$fsBuffer .= $html->tableCells(
										$fsTable,
										array('class' => 'even'),
										array('class' => 'odd')
									);
			$fsBuffer .= '</table>';
		}
		
		// optimize this?
		$userAppLinks = array('primarycache', 'primarycache_mac', 'primarycache_macos', 'dependantOnApp', 'vacmodulecache', 'vacmacmodulecache', 'DemoOfAppID', 'DLCForAppID', 'MustOwnAppToPurchase');
		
		if($data['UserDefined'] === null && !empty($app_info['user_defined'])) {
			$userBuffer = $app_info['user_defined'];
		} else {
			foreach($data['UserDefined'] as $key => $value) {
				if(in_array($key, $userAppLinks)) {
					$value = $format->applink($value);
				}
			
				if(substr($value,0,7) == 'http://') {
					$value = '<a href="' . $value . '">' . $value . '</a>';
				}
			
				$userBuffer .= $key . ' = ' . $value . '<br />';
			}
		}
		
		// 0 is the default, even though it could be technically correct
		$manifestOnly = $app_info['app_of_manifest_only'] > 0 ? $format->applink($app_info['app_of_manifest_only']) :  $app_info['app_of_manifest_only'];
		
		if(isset($reference_cdr)) {
			$bonus = array( $format->column('CDR ID'), $format->cdrlink($reference_cdr) ); 
		} else {
			$bonus = array( $format->column('Last Updated'), $app_info['date_updated'] ); 
		}
		
		echo $html->tableCells(
							array(
								array( $format->column('App ID'), $app_info['app_id'] ),
								$bonus,
								array( $format->column('Name'), $app_info['name'] ),
								array( $format->column('Installation Directory'), $app_info['install_dir'] ),
								array( $format->column('Minimum Cache Size'), $app_info['min_cache_size'] ),
								array( $format->column('Maximum Cache Size'), $app_info['max_cache_size'] ),
								array( $format->column('Launch Options'), $launchBuffer ),
								array( $format->column('On First Launch'), $app_info['on_first_launch'] ),
								array( $format->column('Is Bandwidth Greedy'), $format->boolstring($app_info['is_bandwidth_greedy']) ),
								array( $format->columnLink('Versions', $versionLink), $versionBuffer ),
								array( $format->column('Current Version ID'), $app_info['current_version_id']),
								array( $format->column('Filesystems'), $fsBuffer ),
								array( $format->column('Trickle Version ID'), $app_info['trickle_version_id']),
								array( $format->column('User Defined'), $userBuffer),
								array( $format->column('Beta Version Password'), $app_info['beta_version_password']),
								array( $format->column('Beta Version ID'), $app_info['beta_version_id']),
								array( $format->column('Legacy Installation Dir'), $app_info['legacy_install_dir']),
								array( $format->column('Skip MFP Overwrite'), $format->boolstring($app_info['skip_mfp_overwrite'])),
								array( $format->column('Use Filesystem DVR'), $format->boolstring($app_info['use_filesystem_dvr'])),
								array( $format->column('Manifest Only App'), $format->boolstring($app_info['manifest_only'])),
								array( $format->column('App of Manifest Only Cache'), $manifestOnly )
							),
							null,
							array('class' => 'alt')
						);
	?>
</table>