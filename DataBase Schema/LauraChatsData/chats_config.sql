CREATE TABLE `chats_config` (
  `id` int PRIMARY KEY AUTO_INCREMENT,
  `chatId` varchar(255) NOT NULL,
  `max_warns` int NOT NULL DEFAULT '4',
  `antispam` int NOT NULL DEFAULT '4',
  `as_active` tinyint NOT NULL DEFAULT '0',
  `detect_url` tinyint NOT NULL DEFAULT '0',
  `report_storage` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '0',
  `captcha_button` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'None',
  `nbw_active` tinyint NOT NULL DEFAULT '0',
  `rules` varchar(4096) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'None',
  `custom_hello` varchar(512) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'None',
  `ch_sticker` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'None',
  `punish` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'Mute',
  `cp_ma` int NOT NULL DEFAULT '3',
  `cp_minutes` int NOT NULL DEFAULT '5',
  `night` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'None',
  `state` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'None',
  `settings_state` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'None',
  `state_owner` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '0',
  `gmt` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'None',
  `receive_news` tinyint NOT NULL DEFAULT '1',
  `chat_state` varchar(255) NOT NULL DEFAULT 'State',
  `st_expire` varchar(255) NOT NULL DEFAULT 'None'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

