-- phpMyAdmin SQL Dump
-- version 3.4.0
-- http://www.phpmyadmin.net
--
-- Host: 127.0.0.1:3306

-- Generation Time: Jul 12, 2012 at 05:16 PM
-- Server version: 5.5.12
-- PHP Version: 5.3.6

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

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
  `trickle_version_id` int(11) NOT NULL,
  `user_defined` text NOT NULL,
  `beta_version_password` varchar(255) NOT NULL,
  `beta_version_id` int(11) NOT NULL,
  `legacy_install_dir` varchar(255) NOT NULL,
  `skip_mfp_overwrite` tinyint(1) NOT NULL,
  `use_filesystem_dvr` tinyint(1) NOT NULL,
  `manifest_only` tinyint(1) NOT NULL,
  `app_of_manifest_only` int(11) NOT NULL,
  `sub_count` int(11) NOT NULL,
  `date_updated` datetime NOT NULL,
  PRIMARY KEY (`app_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `apps_subs`
--

CREATE TABLE `apps_subs` (
  `app_id` int(11) NOT NULL,
  `sub_id` int(11) NOT NULL,
  `cdr_id` int(11) NOT NULL,
  `cdr_id_last` int(11) DEFAULT NULL,
  PRIMARY KEY (`app_id`,`sub_id`,`cdr_id`),
  KEY `appid_subid` (`app_id`,`sub_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `app_filesystem`
--

CREATE TABLE `app_filesystem` (
  `app_id` int(11) NOT NULL,
  `cdr_id` int(11) NOT NULL,
  `cdr_id_last` int(11) DEFAULT NULL,
  `app_id_filesystem` int(11) NOT NULL,
  `mount_name` varchar(255) NOT NULL,
  `is_optional` tinyint(1) NOT NULL,
  `platform` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`app_id`,`cdr_id`,`app_id_filesystem`,`mount_name`),
  KEY `cdr_id_last` (`cdr_id_last`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `app_state_capture`
--

CREATE TABLE `app_state_capture` (
  `cdr_id` int(11) NOT NULL,
  `created` tinyint(1) NOT NULL,
  `app_id` int(11) NOT NULL,
  `name` varchar(255) DEFAULT NULL,
  `install_dir` varchar(255) DEFAULT NULL,
  `min_cache_size` smallint(5) DEFAULT NULL,
  `max_cache_size` smallint(5) DEFAULT NULL,
  `launch_options` text,
  `on_first_launch` int(11) DEFAULT NULL,
  `is_bandwidth_greedy` tinyint(1) DEFAULT NULL,
  `current_version_id` int(11) DEFAULT NULL,
  `trickle_version_id` int(11) DEFAULT NULL,
  `user_defined` text,
  `beta_version_password` varchar(255) DEFAULT NULL,
  `beta_version_id` int(11) DEFAULT NULL,
  `legacy_install_dir` varchar(255) DEFAULT NULL,
  `skip_mfp_overwrite` tinyint(1) DEFAULT NULL,
  `use_filesystem_dvr` tinyint(1) DEFAULT NULL,
  `manifest_only` tinyint(1) DEFAULT NULL,
  `app_of_manifest_only` int(11) DEFAULT NULL,
  `sub_count` int(11) DEFAULT NULL,
  PRIMARY KEY (`cdr_id`,`app_id`),
  KEY `created` (`created`),
  KEY `appid` (`app_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `app_version`
--

CREATE TABLE `app_version` (
  `app_id` int(11) NOT NULL,
  `cdr_id` int(11) NOT NULL,
  `cdr_id_last` int(11) DEFAULT NULL,
  `description` varchar(255) NOT NULL,
  `version_id` int(11) NOT NULL,
  `is_not_available` tinyint(1) NOT NULL,
  `launch_option_ids` text NOT NULL,
  `depot_key` varchar(32) NOT NULL,
  `is_encryption_key_available` tinyint(1) NOT NULL,
  `is_rebased` tinyint(1) NOT NULL,
  `is_long_version_roll` tinyint(1) NOT NULL,
  PRIMARY KEY (`app_id`,`cdr_id`,`description`,`version_id`),
  KEY `cdr_id_last` (`cdr_id_last`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `cdr`
--

CREATE TABLE `cdr` (
  `cdr_id` int(11) NOT NULL AUTO_INCREMENT,
  `hash` varchar(40) NOT NULL,
  `version` int(11) NOT NULL,
  `date_updated` datetime NOT NULL,
  `date_processed` datetime NOT NULL,
  `app_count` int(11) NOT NULL,
  `sub_count` int(11) NOT NULL,
  PRIMARY KEY (`cdr_id`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8;

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
  `app_count` int(11) NOT NULL,
  `date_updated` datetime NOT NULL,
  PRIMARY KEY (`sub_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `sub_state_capture`
--

CREATE TABLE `sub_state_capture` (
  `cdr_id` int(11) NOT NULL,
  `created` tinyint(1) NOT NULL,
  `sub_id` int(11) NOT NULL,
  `name` varchar(255) DEFAULT NULL,
  `billing_type` varchar(100) DEFAULT NULL,
  `cost_in_cents` int(11) DEFAULT NULL,
  `period_in_minutes` int(11) DEFAULT NULL,
  `on_subscribe_run_app_id` int(11) DEFAULT NULL,
  `on_subscribe_run_launch_option_index` int(11) DEFAULT NULL,
  `rate_limits` text,
  `discounts` text,
  `is_preorder` tinyint(1) DEFAULT NULL,
  `requires_shipping_address` tinyint(1) DEFAULT NULL,
  `domestic_cost_in_cents` int(11) DEFAULT NULL,
  `international_cost_in_cents` int(11) DEFAULT NULL,
  `required_key_type` int(11) DEFAULT NULL,
  `is_cyber_cafe` tinyint(1) DEFAULT NULL,
  `game_code` int(11) DEFAULT NULL,
  `game_code_description` varchar(255) DEFAULT NULL,
  `is_disabled` tinyint(1) DEFAULT NULL,
  `requires_cd` tinyint(1) DEFAULT NULL,
  `territory_code` int(11) DEFAULT NULL,
  `is_steam3_subscription` tinyint(1) DEFAULT NULL,
  `extended_info` text,
  `app_count` int(11) DEFAULT NULL,
  PRIMARY KEY (`cdr_id`,`sub_id`),
  KEY `created` (`created`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
