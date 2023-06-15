CREATE TABLE `fba_reports` (
  `id` int PRIMARY KEY AUTO_INCREMENT,
  `chat_id` varchar(255) NOT NULL,
  `user_id` varchar(255) NOT NULL,
  `target_id` varchar(255) NOT NULL,
  `reason` varchar(512) NOT NULL,
  `messageId` varchar(255) NOT NULL,
  `stamp` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
