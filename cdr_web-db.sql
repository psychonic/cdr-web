-- phpMyAdmin SQL Dump
-- version 3.3.10
-- http://www.phpmyadmin.net
--
-- Host: 127.0.0.1:3306

-- Generation Time: May 08, 2011 at 07:19 AM
-- Server version: 5.5.11
-- PHP Version: 5.3.6

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";

--
-- Database: `cdr_web`
--

-- --------------------------------------------------------

--
-- Table structure for table `app`
--

CREATE TABLE `app` (
  `app_id` int(11) NOT NULL,
  `name` varchar(255) NOT NULL,
  `install_dir` varchar(255) NOT NULL,
  `min_cache_size` smallint(5) NOT NULL,
  `max_cache_size` smallint(5) NOT NULL,
  `launch_options` text NOT NULL,
  `on_first_launch` int(11) NOT NULL,
  `is_bandwidth_greedy` tinyint(1) NOT NULL,
  `current_version_id` int(11) NOT NULL,
  `filesystems` text NOT NULL,
  `trickle_version_id` int(11) NOT NULL,
  `user_defined` text NOT NULL,
  `beta_version_password` varchar(255) NOT NULL,
  `beta_version_id` int(11) NOT NULL,
  `legacy_install_dir` varchar(255) NOT NULL,
  `skip_mfp_overwrite` tinyint(1) NOT NULL,
  `use_filesystem_dvr` tinyint(1) NOT NULL,
  `manifest_only` tinyint(1) NOT NULL,
  `app_of_manifest_only` int(11) NOT NULL,
  PRIMARY KEY (`app_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

CREATE TABLE `app_state_capture` (
  `cdr_id` int(11) NOT NULL,
  `app_id` int(11) NOT NULL,
  `name` varchar(255) NOT NULL,
  `install_dir` varchar(255) NOT NULL,
  `min_cache_size` smallint(5) NOT NULL,
  `max_cache_size` smallint(5) NOT NULL,
  `launch_options` text NOT NULL,
  `on_first_launch` int(11) NOT NULL,
  `is_bandwidth_greedy` tinyint(1) NOT NULL,
  `current_version_id` int(11) NOT NULL,
  `filesystems` text NOT NULL,
  `trickle_version_id` int(11) NOT NULL,
  `user_defined` text NOT NULL,
  `beta_version_password` varchar(255) NOT NULL,
  `beta_version_id` int(11) NOT NULL,
  `legacy_install_dir` varchar(255) NOT NULL,
  `skip_mfp_overwrite` tinyint(1) NOT NULL,
  `use_filesystem_dvr` tinyint(1) NOT NULL,
  `manifest_only` tinyint(1) NOT NULL,
  `app_of_manifest_only` int(11) NOT NULL,
  PRIMARY KEY (`cdr_id`,`app_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `app`
--


-- --------------------------------------------------------

--
-- Table structure for table `app_filesystem`
--

CREATE TABLE `app_filesystem` (
  `app_id` int(11) NOT NULL,
  `cdr_id` int(11) NOT NULL,
  `cdr_id_last` int(11) NOT NULL,
  `app_id_filesystem` int(11) NOT NULL,
  `mount_name` varchar(255) NOT NULL,
  `is_optional` tinyint(1) NOT NULL,
  PRIMARY KEY (`app_id`,`cdr_id`),
  KEY `cdr_id_last` (`cdr_id_last`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `app_filesystem`
--


-- --------------------------------------------------------

--
-- Table structure for table `app_version`
--

CREATE TABLE `app_version` (
  `app_id` int(11) NOT NULL,
  `cdr_id` int(11) NOT NULL,
  `cdr_id_last` int(11) NOT NULL,
  `description` text NOT NULL,
  `version_id` int(11) NOT NULL,
  `is_not_available` tinyint(1) NOT NULL,
  `launch_option_ids` text NOT NULL,
  `depot_key` text NOT NULL,
  `is_encryption_key_available` tinyint(1) NOT NULL,
  `is_rebased` tinyint(1) NOT NULL,
  `is_long_version_roll` tinyint(1) NOT NULL,
  PRIMARY KEY (`app_id`,`cdr_id`),
  KEY `cdr_id_last` (`cdr_id_last`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `app_version`
--


-- --------------------------------------------------------

--
-- Table structure for table `cdr`
--

CREATE TABLE `cdr` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `hash` varchar(20) NOT NULL,
  `version` int(11) NOT NULL,
  `date_updated` datetime NOT NULL,
  `date_processed` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 AUTO_INCREMENT=1 ;

--
-- Dumping data for table `cdr`
--


-- --------------------------------------------------------

--
-- Table structure for table `sub`
--

CREATE TABLE `sub` (
  `sub_id` int(11) NOT NULL,
  `name` varchar(255) NOT NULL,
  `billing_type` varchar(100) NOT NULL,
  `cost_in_cents` int(11) NOT NULL,
  `period_in_minutes` int(11) NOT NULL,
  `on_subscribe_run_app_id` int(11) NOT NULL,
  `on_subscribe_run_launch_option_index` int(11) NOT NULL,
  `rate_limits` text NOT NULL,
  `discounts` text NOT NULL,
  `is_preorder` tinyint(1) NOT NULL,
  `requires_shipping_address` tinyint(1) NOT NULL,
  `domestic_cost_in_cents` int(11) NOT NULL,
  `international_cost_in_cents` int(11) NOT NULL,
  `required_key_type` int(11) NOT NULL,
  `is_cyber_cafe` tinyint(1) NOT NULL,
  `game_code` int(11) NOT NULL,
  `game_code_description` varchar(255) NOT NULL,
  `is_disabled` tinyint(1) NOT NULL,
  `requires_cd` tinyint(1) NOT NULL,
  `territory_code` int(11) NOT NULL,
  `is_steam3_subscription` tinyint(1) NOT NULL,
  `extended_info` text NOT NULL,
  PRIMARY KEY (`sub_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

CREATE TABLE `sub_state_capture` (
  `cdr_id` int(11) NOT NULL,
  `sub_id` int(11) NOT NULL,
  `name` varchar(255) NOT NULL,
  `billing_type` varchar(100) NOT NULL,
  `cost_in_cents` int(11) NOT NULL,
  `period_in_minutes` int(11) NOT NULL,
  `on_subscribe_run_app_id` int(11) NOT NULL,
  `on_subscribe_run_launch_option_index` int(11) NOT NULL,
  `rate_limits` text NOT NULL,
  `discounts` text NOT NULL,
  `is_preorder` tinyint(1) NOT NULL,
  `requires_shipping_address` tinyint(1) NOT NULL,
  `domestic_cost_in_cents` int(11) NOT NULL,
  `international_cost_in_cents` int(11) NOT NULL,
  `required_key_type` int(11) NOT NULL,
  `is_cyber_cafe` tinyint(1) NOT NULL,
  `game_code` int(11) NOT NULL,
  `game_code_description` varchar(255) NOT NULL,
  `is_disabled` tinyint(1) NOT NULL,
  `requires_cd` tinyint(1) NOT NULL,
  `territory_code` int(11) NOT NULL,
  `is_steam3_subscription` tinyint(1) NOT NULL,
  `extended_info` text NOT NULL,
  PRIMARY KEY (`cdr_id`,`sub_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `sub`
--

CREATE TABLE `apps_subs` (
  `app_id` int(11) NOT NULL,
  `sub_id` int(11) NOT NULL,
  `cdr_id` int(11) NOT NULL,
  KEY `app_id` (`app_id`,`sub_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;