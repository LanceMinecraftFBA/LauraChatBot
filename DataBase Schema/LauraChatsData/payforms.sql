CREATE TABLE `payforms` (
  `id` int PRIMARY KEY AUTO_INCREMENT,
  `chatId` varchar(255) NOT NULL,
  `billId` varchar(255) NOT NULL,
  `expire` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
