<?php
include 'db_connect.php';

$run_id = $_POST['run_id'];
$enemy_id = $_POST['enemy_id'];
$health = $_POST['health'];
$x = $_POST['x']; $y = $_POST['y']; $z = $_POST['z'];

$stmt = $conn->prepare("INSERT INTO EnemyHit_Event (GameRun_ID, Enemy_ID, Remaining_Health, Pos_x, Pos_y, Pos_z) VALUES (?, ?, ?, ?, ?, ?)");
$stmt->bind_param("iidddd", $run_id, $enemy_id, $health, $x, $y, $z);

if ($stmt->execute()) echo "Enemy hit recorded";
else echo "Error: " . $stmt->error;

$conn->close();
?>