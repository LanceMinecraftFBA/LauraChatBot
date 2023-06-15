CREATE TABLE `blocked_chats` (
  `id` int PRIMARY KEY AUTO_INCREMENT,
  `chat_id` varchar(255) NOT NULL,
  `reason` int NOT NULL,
  `expire` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
