CREATE TABLE `rating_control` (
  `id` int PRIMARY KEY AUTO_INCREMENT,
  `userId` varchar(255) NOT NULL,
  `targetUser` varchar(255) NOT NULL,
  `count` int NOT NULL,
  `expire` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
