package cmd

import (
	"fmt"

	"github.com/youngwoocho02/unity-cli/internal/client"
)

func profilerCmd(args []string, send sendFn) (*client.CommandResponse, error) {
	if len(args) == 0 {
		args = []string{"hierarchy"}
	}

	action := args[0]
	flags := parseSubFlags(args[1:])

	switch action {
	case "hierarchy":
		params := map[string]interface{}{}
		setInt(flags, params, "parent", "parent_id")
		setInt(flags, params, "frame", "frame")
		setInt(flags, params, "thread", "thread_index")
		setFloat(flags, params, "min", "min_time")
		setStr(flags, params, "sort", "sort_by")
		setInt(flags, params, "max", "max_items")
		setInt(flags, params, "depth", "depth")
		return send("profiler_hierarchy", params)

	case "enable":
		return send("manage_profiler", map[string]interface{}{"action": "enable"})

	case "disable":
		return send("manage_profiler", map[string]interface{}{"action": "disable"})

	case "status":
		return send("manage_profiler", map[string]interface{}{"action": "status"})

	case "clear":
		return send("manage_profiler", map[string]interface{}{"action": "clear"})

	default:
		return nil, fmt.Errorf("unknown profiler action: %s\nAvailable: hierarchy, enable, disable, status, clear", action)
	}
}
