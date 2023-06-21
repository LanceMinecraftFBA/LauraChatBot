CREATE TABLE `uadmins_chat_state` (
  `id` int PRIMARY KEY AUTO_INCREMENT,
  `userId` varchar(255) NOT NULL,
  `messageId` int NOT NULL,
  `chatId` varchar(255) NOT NULL,
  `state` varchar(255) NOT NULL,
  `expire` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
