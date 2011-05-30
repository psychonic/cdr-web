<?php echo $form->create('Application',array('action'=>'search'));?>
	<?php
		echo $form->input('app_id', array('label' => __('App ID', true), 'type' => 'text'));
		echo $form->input('name',  array('label' => __('Name', true), 'type' => 'text'));
		echo $form->submit('Search');
	?>
<?php echo $form->end();?>