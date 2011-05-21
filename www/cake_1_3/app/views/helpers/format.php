<?php
/**
 * CDR format helpers
*/

class FormatHelper extends AppHelper {

	function __construct() {
		parent::__construct();
	}

	function applink($html, $appid, $text = null, $user = null) {
		if ($appid < 0) {
			return ($text == null ? $appid : $text);
		}
		
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
	
	function cdrlink($html, $subid, $text = null, $user = null) {
		$target = array('controller' => 'contentrecords', 'action' => 'view', 'id' => $subid);
		
		if($user != null) {
			$target = array_merge($target, $user);
		}
		
		return $html->link($text == null ? $subid : $text, $target);
	}
	
	function column($key, $class='column') {
		return array(__($key, true), array('class' => $class));
	}
	
	function columnLiteral($key, $class='column') {
		return array($key, array('class' => $class));
	}
	
	function columnLink($key, $link) {
		return array(__($key, true) . ' (' . $link . ')', array('class' => 'column'));
	}
	
	function boolstring($value) {
			return $value ? 'true' : 'false';
	}
}
