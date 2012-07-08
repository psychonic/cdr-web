<?php
class AppError extends ErrorHandler 
{
	function noItemFound($params)
	{
		$this->controller->set('item_id', $params['item_id']);
		$this->_outputMessage('no_item_found');
	}
}
?>