<?php
/**
 * CDR format helpers
*/

class FormatHelper extends AppHelper {

	function __construct() {
		parent::__construct();
	}

	function applink($html, $appid, $text = null, $user = null) {
		$target = array('controller' => 'applications', 'action' => 'view', 'id' => $appid);
		
		if($user != null) {
			$target = array_merge($target, $user);
		}
		
		return $html->link($text == null ? $appid : $text, $target);
	}
	
	function sublink($html, $subid, $text = null, $user = null) {
		$target = array('controller' => 'subscriptions', 'action' => 'view', 'id' => $subid);
		
		if($user != null) {
			$target = array_merge($target, $user);
		}
		
		return $html->link($text == null ? $subid : $text, $target);
	}
	
	function column($key) {
		return array(__($key, true), array('class' => 'column'));
	}
	
	function columnLiteral($key) {
		return array($key, array('class' => 'column'));
	}
	
	function columnLink($key, $link) {
		return array(__($key, true) . ' (' . $link . ')', array('class' => 'column'));
	}
	
	function tostring($value) {
		if(is_bool($value)) {
			return $value ? 'true' : 'false';
		}
		
		return (string)$value;
	}
}
