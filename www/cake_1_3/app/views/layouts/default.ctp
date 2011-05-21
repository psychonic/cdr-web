<!DOCTYPE html
	PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
	 "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
		
<head>
	<?php echo $this->Html->charset(); ?>
	<title><?php __('CDR Database'); ?> - <?php echo $title_for_layout; ?></title>
	<?php
		echo $this->Html->meta('icon');
		echo $this->Html->css('cdr.main');
		echo $scripts_for_layout;
	?>
</head>
<body>
	<div id="wrapper">
	
		<div id="header">
			<div id="logo">
				<?php 
					echo $this->Html->link(
							$this->Html->image(
									'logo.png',
									array('alt' => __('Open Steamworks CDR', true))
							),
							'/',
							array('escape' => false, 'class' => 'noborder')
					);
				?>
			</div>
			
			<div id="nav">
				<div class="stack">
					<h3>main</h3>
					<ul>
						<li><?php echo $this->Html->link(__('home', true), '/'); ?></li>
						<li><?php echo $this->Html->link(__('about', true), array('controller' => 'pages', 'action' => 'display', 'about')); ?></li>
					<ul>
				</div>
				<div class="stack">
					<h3>lists</h3>
					<ul>
						<li><?php echo $this->Html->link(__('cdr', true), '/'); ?></li>
						<li><?php echo $this->Html->link(__('apps', true), array('controller' => 'applications', 'action' => 'index')); ?></li>
						<li><?php echo $this->Html->link(__('subs', true), array('controller' => 'subscriptions', 'action' => 'index')); ?></li>
					</ul>
				</div>
				<div class="stack">
					<h3>search</h3>
					<ul>
						<li><?php echo $this->Html->link(__('apps', true), '/'); ?></li>
						<li><?php echo $this->Html->link(__('subs', true), '/'); ?></li>
					</ul>
				</div>
			</div>
			<div>
				<h3><?php echo $title_for_layout; ?></h3>
			</div>
		</div>
		
		<?php
			if(isset($layout_menuitems)) {
		?>
		<div id="submenu_container">
		<?php
				foreach($layout_menuitems as $action) {
		?>
		<div class="submenu"> <?php echo $html->link($action, array('controller' => 'applications', 'action' => $action, 'id' => $reference_id), array('class' => 'noborder')); ?> </div>
		<?php
				}
		?>
		</div>
		<?php
			}
		?>
		
		<div id="content">
			<?php echo $content_for_layout ?>
		</div>
	</div>

	<?php echo $this->element('sql_dump'); ?>
</body>
</html>