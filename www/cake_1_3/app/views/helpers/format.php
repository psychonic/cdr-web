<?php
/**
 * CDR format helpers
*/

class FormatHelper extends AppHelper {

	var $helpers = array('Html');
	
	function __construct() {
		parent::__construct();
	}

	function link($id, $controller, $text = null, $user = null) {
		if($id < 0) {
			return ($text == null ? $id : $text);
		}
		
		$target = array('controller' => $controller, 'action' => 'view', 'id' => $id);
		
		if($user != null) {
			$target = array_merge($target, $user);
		}
		
		return $this->Html->link($text == null ? $id : $text, $target);
	}
	
	function applink($appid, $text = null, $user = null) {
		return $this->link($appid, 'applications', $text, $user);
	}
	
	function sublink($subid, $text = null, $user = null) {
		return $this->link($subid, 'subscriptions', $text, $user);
	}
	
	function cdrlink($cdrid, $text = null, $user = null) {
		return $this->link($cdrid, 'contentrecords', $text, $user);
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
