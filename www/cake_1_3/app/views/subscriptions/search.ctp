<?php echo $form->create('Subscription',array('action'=>'search'));?>
	<?php
		echo $form->input('sub_id', array('label' => __('Sub ID', true) . ': ', 'type' => 'text'));
		echo $form->input('name',  array('label' => __('Name', true) . ': ', 'type' => 'text'));
		echo $form->submit('Search');
	?>
<?php echo $form->end();?>