CREATE TABLE `report_storage_request` (
  `id` int PRIMARY KEY AUTO_INCREMENT,
  `targetChat` varchar(255) NOT NULL,
  `outputChat` varchar(255) NOT NULL,
  `expire` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
